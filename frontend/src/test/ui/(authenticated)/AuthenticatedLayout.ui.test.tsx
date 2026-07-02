import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';

const mockUseSession = vi.fn();

vi.mock('@/lib/authentication/useSession', () => ({
  useSession: () => mockUseSession(),
}));

describe('AuthenticatedLayout (integration)', () => {
  beforeEach(() => {
    mockUseSession.mockReset();
  });

  async function loadPage() {
    const mod = await import('@/app/(authenticated)/layout');
    return mod.default;
  }

  it('shows children when authenticated', async () => {
    mockUseSession.mockReturnValue({
      isAuthenticated: true,
      isUnavailable: false,
      error: null,
      refresh: vi.fn(),
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByText('child')).toBeInTheDocument();
  });

  it('shows login message when unauthenticated', async () => {
    mockUseSession.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: false,
      error: null,
      refresh: vi.fn(),
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByText(/login to view/i)).toBeInTheDocument();
  });

  it('behaves like unauthenticated when API is unavailable', async () => {
    mockUseSession.mockReturnValue({
      isAuthenticated: false,
      isUnavailable: true,
      error: 'authentication service unavailable',
      refresh: vi.fn(),
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByText(/login to view/i)).toBeInTheDocument();
  });
});
