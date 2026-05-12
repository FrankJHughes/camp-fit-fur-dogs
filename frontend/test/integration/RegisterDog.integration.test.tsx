import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { fillDogForm } from '../helpers/dogs/fillDogForm.test';

// IMPORTANT: mock next/navigation BEFORE importing the page
const pushMock = vi.fn();
vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

// IMPORTANT: mock the API client BEFORE importing the page
import { apiClientMock } from '../setup';

describe('Register Dog (integration)', () => {
  beforeEach(() => {
    vi.resetModules();          // force fresh component tree
    pushMock.mockClear();       // reset navigation
    apiClientMock.get.mockClear();
    apiClientMock.post.mockClear();
    apiClientMock.put.mockClear();
    apiClientMock.delete.mockClear();
  });

  async function loadPage() {
    // dynamically import AFTER mocks are applied
    const mod = await import('@/app/dogs/register/page');
    return mod.default;
  }

  it('submits through the real API client and navigates on success', async () => {
    const RegisterDogPage = await loadPage();
    const user = userEvent.setup();

    render(<RegisterDogPage />);

    await fillDogForm(user);

    await user.click(screen.getByRole('button', { name: /register/i }));

    await waitFor(() => {
      expect(pushMock).toHaveBeenCalledWith('/dogs/register/success');
    });

    expect(apiClientMock.post).toHaveBeenCalledWith(
      '/dogs/register',
      {
        name: 'Buddy',
        breed: 'Golden Retriever',
        dateOfBirth: '2023-06-15',
        sex: 'Male',
      }
    );
  });

  it('displays server validation errors without navigating', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: { name: ['Name is already taken'] },
      },
    });

    const RegisterDogPage = await loadPage();
    const user = userEvent.setup();

    render(<RegisterDogPage />);

    await fillDogForm(user);

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(await screen.findByText('Name is already taken')).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });

  it('displays a network error when the server is unreachable', async () => {
    apiClientMock.post.mockResolvedValueOnce({
      ok: false,
      error: {
        type: 'network',
        message: 'Failed to fetch',
      },
    });

    const RegisterDogPage = await loadPage();
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
