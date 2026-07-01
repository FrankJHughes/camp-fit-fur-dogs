import { describe, it, expect, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { pushMock } from '@/test/helpers/mockRouter';
import {
  useApiQueryMock,
  setUseApiQueryReturn,
  resetUseApiQueryMock,
} from '@/test/helpers/mockUseApiQuery';

describe('DogsPage (UI)', () => {
  beforeEach(() => {
    resetUseApiQueryMock();
    pushMock.mockClear();
  });

  async function loadPage() {
    const mod = await import('@/app/(authenticated)/dogs/page');
    return mod.default;
  }

  it('shows loading state', async () => {
    setUseApiQueryReturn({ status: 'loading' });

    const DogsPage = await loadPage();
    render(<DogsPage />);

    expect(screen.getByText(/loading/i)).toBeInTheDocument();
  });

  it('renders list of dogs on success', async () => {
    setUseApiQueryReturn({
      status: 'success',
      data: {
        dogs: [
          { id: '1', name: 'Rex', breed: 'Labrador' },
          { id: '2', name: 'Bella', breed: 'Husky' },
        ],
      },
    });

    const DogsPage = await loadPage();
    render(<DogsPage />);

    // Match accessible link names
    expect(screen.getByRole('link', { name: /Rex/i })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /Bella/i })).toBeInTheDocument();
  });

  it('renders empty state when no dogs exist', async () => {
    setUseApiQueryReturn({
      status: 'success',
      data: { dogs: [] },
    });

    const DogsPage = await loadPage();
    render(<DogsPage />);

    expect(screen.getByText(/no dogs registered yet/i)).toBeInTheDocument();
  });

  it('renders error state', async () => {
    setUseApiQueryReturn({
      status: 'error',
      error: 'Something went wrong',
    });

    const DogsPage = await loadPage();
    render(<DogsPage />);

    expect(screen.getByText(/something went wrong/i)).toBeInTheDocument();
  });

  it('navigates to register dog page when clicking Register button', async () => {
    setUseApiQueryReturn({
      status: 'success',
      data: { dogs: [] },
    });

    const DogsPage = await loadPage();
    const user = userEvent.setup();

    render(<DogsPage />);

    await user.click(screen.getByRole('button', { name: /register/i }));

    expect(pushMock).toHaveBeenCalledWith('/dogs/register');
  });
});
