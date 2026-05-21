import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { pushMock } from '@/test/helpers/mockRouter';
import { fillCreateAccountForm } from '@/test/helpers/account/fillCreateAccountForm';

// IMPORTANT: mock the API client BEFORE importing the page
import { apiClientMock } from '@/test/setup';

describe('CreateAccountPage (UI)', () => {
  beforeEach(() => {
    vi.resetModules();
    pushMock.mockClear();
    apiClientMock.post.mockClear();
  });

  async function loadPage() {
    const mod = await import('@/app/(auth)/create-account/page');
    return mod.default;
  }

  it('submits the form and navigates on success', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: true,
      data: { success: true },
    });

    const CreateAccountPage = await loadPage();
    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);

    await user.click(screen.getByRole('button', { name: /create account/i }));

    await waitFor(() => {
      expect(pushMock).toHaveBeenCalledWith('/create-account/success');
    });

    expect(apiClientMock.post).toHaveBeenCalledWith('/api/customers', {
      firstName: 'Frank',
      lastName: 'Hughes',
      email: 'frank@example.com',
      phone: '916-555-1234', // updated to match helper + schema
      password: 'Password123!',
    });
  });

  it('shows server validation errors', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: { email: ['Email already exists'] },
      },
    });

    const CreateAccountPage = await loadPage();
    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(await screen.findByText('Email already exists')).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });

  it('shows a network error when the server is unreachable', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'network',
        message: 'Failed to fetch',
      },
    });

    const CreateAccountPage = await loadPage();
    const user = userEvent.setup();

    render(<CreateAccountPage />);

    await fillCreateAccountForm(user);

    await user.click(screen.getByRole('button', { name: /create account/i }));

    expect(
      await screen.findByText('A network error occurred. Please try again.')
    ).toBeInTheDocument();

    expect(pushMock).not.toHaveBeenCalled();
  });
});
