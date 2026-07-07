import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';

const mockUseIdentityState = vi.fn();

vi.mock('@/lib/identity/useIdentityState', () => ({
  useIdentityState: () => mockUseIdentityState(),
}));

async function loadLayout() {
  const mod = await import('@/app/(authenticated)/layout');
  return mod.default;
}

describe('(authenticated)/layout', () => {
  beforeEach(() => mockUseIdentityState.mockReset());

  it('renders children when authenticated', async () => {
    mockUseIdentityState.mockReturnValue({
      isAuthenticated: true,
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText('child')).toBeInTheDocument();
  });

  it('shows login message when unauthenticated', async () => {
    mockUseIdentityState.mockReturnValue({
      isAuthenticated: false,
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText(/login to view/i)).toBeInTheDocument();
  });
});
