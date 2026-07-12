import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';

// Mock identity API calls
vi.mock('@/api/identity/identify', () => ({
  identify: vi.fn(),
}));

vi.mock('@/api/identity/login', () => ({
  login: vi.fn(),
}));

vi.mock('@/api/identity/logout', () => ({
  logout: vi.fn(),
}));

import { identify } from '@/api/identity/identify';
import { login } from '@/api/identity/login';
import { logout } from '@/api/identity/logout';

async function loadLayout() {
  const mod = await import('@/app/layout');
  return mod.default;
}

// Helper: wait for identity transition (2 seconds + buffer)
function waitIdentityTransition() {
  return new Promise(resolve => setTimeout(resolve, 2100));
}

describe('RootLayout — identity + authentication integration', () => {
  const mockIdentify = vi.mocked(identify);
  const mockLogin = vi.mocked(login);
  const mockLogout = vi.mocked(logout);

  beforeEach(() => {
    mockIdentify.mockReset();
    mockLogin.mockReset();
    mockLogout.mockReset();
  });

  it('shows authenticated identity and logout button after identify succeeds', async () => {
    mockIdentify.mockResolvedValue({
      success: true,
      data: { name: 'Harry Styles' },
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    await waitIdentityTransition();

    expect(screen.getByText(/harry styles/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /log out/i })).toBeInTheDocument();
  });

  it('shows anonymous identity and login button when identify fails', async () => {
    mockIdentify.mockResolvedValue({
      success: false,
      error: 'unauthenticated',
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    await waitIdentityTransition();

    expect(screen.getByText(/anonymous/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /log in/i })).toBeInTheDocument();
  });

  it('performs login and redirects when login succeeds', async () => {
    mockIdentify.mockResolvedValue({
      success: false,
      error: 'unauthenticated',
    });

    mockLogin.mockResolvedValue({
      success: true,
      data: { nextUrl: '/after-login' },
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    await waitIdentityTransition();

    const loginButton = await screen.findByRole('button', { name: /log in/i });

    delete (window as any).location;
    (window as any).location = { href: '' };

    loginButton.click();

    await waitIdentityTransition();

    expect(window.location.href).toBe('/after-login');
  });

  it('performs logout and redirects when logout succeeds', async () => {
    mockIdentify.mockResolvedValue({
      success: true,
      data: { name: 'Harry Styles' },
    });

    mockLogout.mockResolvedValue({
      success: true,
      data: { nextUrl: '/after-logout' },
    });

    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);

    await waitIdentityTransition();

    const logoutButton = await screen.findByRole('button', { name: /log out/i });

    delete (window as any).location;
    (window as any).location = { href: '' };

    logoutButton.click();

    await waitIdentityTransition();

    expect(window.location.href).toBe('/after-logout');
  });
});
