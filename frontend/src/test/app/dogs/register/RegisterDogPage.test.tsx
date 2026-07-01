import { pushMock } from '@/test/helpers/mockRouter';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { apiClientMock } from '@/test/setup';
import { fillDogForm } from '@/test/helpers/dogs/fillDogForm';
import RegisterDogPage from '@/app/(authenticated)/dogs/register/page';

describe('Register Dog Page', () => {
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

    render(<RegisterDogPage />);

    await fillDogForm(user);

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(apiClientMock.post).toHaveBeenCalledWith('/dogs/register', {
      name: 'Buddy',
      breed: 'Golden Retriever',
      dateOfBirth: '2023-06-15',
      sex: 'Male',
    });

    expect(pushMock).toHaveBeenCalledWith('/dogs/register/success');
  });

  it('displays validation errors returned by the API', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: {
          name: ['Name is required'],
          breed: ['Breed is required'],
        },
      },
    });

    const user = userEvent.setup();

    render(<RegisterDogPage />);

    await fillDogForm(user);
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(await screen.findByText('Name is required')).toBeInTheDocument();
    expect(screen.getByText('Breed is required')).toBeInTheDocument();
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

    render(<RegisterDogPage />);

    await fillDogForm(user);
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(screen.getByRole('button', { name: /register/i })).toBeDisabled();

    resolveApi!({ ok: true, data: {} });

    await waitFor(() =>
      expect(
        screen.getByRole('button', { name: /register/i })
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

    render(<RegisterDogPage />);

    await fillDogForm(user);
    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(
      await screen.findByText('A network error occurred. Please try again.')
    ).toBeInTheDocument();

    expect(pushMock).not.toHaveBeenCalled();
  });
});
