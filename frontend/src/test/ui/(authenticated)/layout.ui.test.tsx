import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';

const mockUseIdentity = vi.fn();

vi.mock('@/lib/identity/useIdentity', () => ({
  useIdentity: () => mockUseIdentity(),
}));

async function loadLayout() {
  const mod = await import('@/app/(authenticated)/layout');
  return mod.default;
}

describe('(authenticated)/layout', () => {
  beforeEach(() => mockUseIdentity.mockReset());

  it('renders children when authenticated', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: true,
      isUnavailable: false,
      error: null,
      user: { name: 'Harry Styles' },
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText('child')).toBeInTheDocument();
  });

  it('shows login message when unauthenticated', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: false,
      error: null,
      user: null,
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText(/login to view/i)).toBeInTheDocument();
  });

  it('shows login message when unavailable', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: true,
      error: null,
      user: null,
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText(/login to view/i)).toBeInTheDocument();
  });
});
