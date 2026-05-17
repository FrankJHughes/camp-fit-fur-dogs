import { pushMock } from '@/test/helpers/mockRouter';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { apiClientMock } from '@/test/setup';
import { fillCreateAccountForm } from '@/test/helpers/account/fillCreateAccountForm';
import CreateAccountPage from '@/app/(auth)/create-account/page';

describe('Create Account Page', () => {
  beforeEach(() => {
    pushMock.mockClear();
    apiClientMock.post.mockClear();
  });

  it('calls the API when the form is submitted', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: true,
      data: {},
    });

    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);

    await user.click(
      screen.getByRole('button', { name: /create account/i })
    );

    expect(apiClientMock.post).toHaveBeenCalledWith('/account/create', {
      email: 'frank@example.com',
      password: 'Password123!',
      confirmPassword: 'Password123!',
    });

    expect(pushMock).toHaveBeenCalledWith('/create-account/success');
  });

  it('displays validation errors returned by the API', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: {
          email: ['Email is required'],
          password: ['Password must be at least 8 characters'],
        },
      },
    });

    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);
    await user.click(
      screen.getByRole('button', { name: /create account/i })
    );

    expect(await screen.findByText('Email is required')).toBeInTheDocument();
    expect(
      screen.getByText('Password must be at least 8 characters')
    ).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });

  it('disables the submit button while the API call is in flight', async () => {
    let resolveApi: (value: any) => void;

    apiClientMock.post.mockImplementationOnce(
      () =>
        new Promise((resolve) => {
          resolveApi = resolve;
        })
    );

    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);
    await user.click(
      screen.getByRole('button', { name: /create account/i })
    );

    expect(
      screen.getByRole('button', { name: /create account/i })
    ).toBeDisabled();

    resolveApi!({ ok: true, data: {} });

    await waitFor(() =>
      expect(
        screen.getByRole('button', { name: /create account/i })
      ).not.toBeDisabled()
    );
  });

  it('displays a form-level error when the API returns a network error', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'network',
        message: 'Failed to fetch',
      },
    });

    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);
    await user.click(
      screen.getByRole('button', { name: /create account/i })
    );

    expect(
      await screen.findByText(
        'A network error occurred. Please try again.'
      )
    ).toBeInTheDocument();

    expect(pushMock).not.toHaveBeenCalled();
  });
});
