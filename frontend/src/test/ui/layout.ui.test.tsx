import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';

const mockUseIdentity = vi.fn();

vi.mock('@/lib/identity/useIdentity', () => ({
  useIdentity: () => mockUseIdentity(),
}));

vi.mock('@/api/authentication/login', () => ({ login: vi.fn() }));
vi.mock('@/api/authentication/logout', () => ({ logout: vi.fn() }));

async function loadLayout() {
  const mod = await import('@/app/layout');
  return mod.default;
}

describe('RootLayout — identity + authentication integration', () => {
  it('shows identity and logout button when authenticated', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: true,
      isUnavailable: false,
      isLoading: false,
      error: null,
      user: { name: 'Harry Styles' },
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText(/you are harry styles/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /logout/i })).toBeInTheDocument();
  });

  it('shows anonymous identity and login button when unauthenticated', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: false,
      isLoading: false,
      error: null,
      user: null,
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    expect(screen.getByText(/you are anonymous/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  it('hides identity and auth actions when service is unavailable', async () => {
    mockUseIdentity.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: true,
      isLoading: false,
      error: null,
      user: null,
      refresh: vi.fn(),
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    // Banner must show unavailable message
    expect(
      screen.getByText(/authentication service unavailable/i)
    ).toBeInTheDocument();

    // Identity must NOT appear
    expect(screen.queryByText(/you are/i)).toBeNull();

    // No login/logout buttons
    expect(screen.queryByRole('button', { name: /login/i })).toBeNull();
    expect(screen.queryByRole('button', { name: /logout/i })).toBeNull();
  });
});
