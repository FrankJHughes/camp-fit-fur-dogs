import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { pushMock, paramsMock } from '@/test/helpers/mockRouter';
import {
  setUseApiQueryReturn,
  resetUseApiQueryMock,
} from '@/test/helpers/mockUseApiQuery';

// --- Mock useRemoveDog ---
const openMock = vi.fn();
const dialogPropsMock = {};
let removeErrorMock: string | null = null;

vi.mock('@/lib/dogs/useRemoveDog', () => ({
  useRemoveDog: () => ({
    open: openMock,
    dialogProps: dialogPropsMock,
    error: removeErrorMock,
  }),
}));

describe('GetDogProfilePage (UI)', () => {
  beforeEach(() => {
    resetUseApiQueryMock();
    pushMock.mockClear();
    openMock.mockClear();
    removeErrorMock = null;

    // Set default route param
    paramsMock.id = 'dog-1';
  });

  async function loadPage() {
    const mod = await import('@/app/(authenticated)/dogs/[id]/page');
    return mod.default;
  }

  it('shows loading state', async () => {
    setUseApiQueryReturn({ status: 'loading' });

    const Page = await loadPage();
    render(<Page />);

    expect(screen.getByText(/loading/i)).toBeInTheDocument();
  });

  it('shows not-found state', async () => {
    setUseApiQueryReturn({ status: 'not-found' });

    const Page = await loadPage();
    render(<Page />);

    expect(
      screen.getByRole('heading', { name: /couldn't find that dog/i })
    ).toBeInTheDocument();
  });

  it('shows error state', async () => {
    setUseApiQueryReturn({
      status: 'error',
      error: 'Something went wrong',
    });

    const Page = await loadPage();
    render(<Page />);

    expect(screen.getByText(/something went wrong/i)).toBeInTheDocument();
  });

  it('renders dog profile on success', async () => {
    setUseApiQueryReturn({
      status: 'success',
      data: {
        name: 'Rex',
        breed: 'Labrador',
        dateOfBirth: '2020-01-01',
        sex: 'Male',
      },
    });

    const Page = await loadPage();
    render(<Page />);

    expect(await screen.findByRole('heading', { name: /rex/i })).toBeInTheDocument();
    expect(screen.getByText(/labrador/i)).toBeInTheDocument();
  });

  it('navigates to edit page when clicking Edit', async () => {
    const user = userEvent.setup();

    setUseApiQueryReturn({
      status: 'success',
      data: {
        name: 'Rex',
        breed: 'Labrador',
        dateOfBirth: '2020-01-01',
        sex: 'Male',
      },
    });

    const Page = await loadPage();
    render(<Page />);

    await user.click(screen.getByRole('button', { name: /edit/i }));

    expect(pushMock).toHaveBeenCalledWith('/dogs/dog-1/edit');
  });

  it('opens remove dialog when clicking Remove', async () => {
    const user = userEvent.setup();

    setUseApiQueryReturn({
      status: 'success',
      data: {
        name: 'Rex',
        breed: 'Labrador',
        dateOfBirth: '2020-01-01',
        sex: 'Male',
      },
    });

    const Page = await loadPage();
    render(<Page />);

    await user.click(screen.getByRole('button', { name: /remove/i }));

    expect(openMock).toHaveBeenCalled();
  });

  it('renders removeDog error when present', async () => {
    removeErrorMock = 'Failed to remove dog';

    setUseApiQueryReturn({
      status: 'success',
      data: {
        name: 'Rex',
        breed: 'Labrador',
        dateOfBirth: '2020-01-01',
        sex: 'Male',
      },
    });

    const Page = await loadPage();
    render(<Page />);

    expect(screen.getByRole('alert')).toHaveTextContent(/failed to remove dog/i);
  });
});
