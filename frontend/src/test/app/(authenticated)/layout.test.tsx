import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';

const mockUseSession = vi.fn();

vi.mock('@/lib/authentication/useSession', () => ({
  useSession: () => mockUseSession(),
}));

describe('(authenticated) layout', () => {
  beforeEach(() => {
    mockUseSession.mockReset();
  });

  async function renderLayout() {
    const { default: Layout } = await import('@/app/(authenticated)/layout');

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );
  }

  it('renders children when authenticated', async () => {
    mockUseSession.mockReturnValue({
      isAuthenticated: true,
      isLoading: false,
      error: null,
      refresh: vi.fn(),
    });

    await renderLayout();

    expect(await screen.findByText('child')).toBeInTheDocument();
  });

  it('shows login message when unauthenticated', async () => {
    mockUseSession.mockReturnValue({
      isAuthenticated: false,
      isLoading: false,
      error: null,
      refresh: vi.fn(),
    });

    await renderLayout();

    expect(await screen.findByText(/login to view/i)).toBeInTheDocument();
  });
});
