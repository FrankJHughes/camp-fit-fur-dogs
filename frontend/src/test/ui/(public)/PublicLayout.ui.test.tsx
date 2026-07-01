import { describe, it, expect, beforeEach, vi, MockedFunction } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// ------------------------------------------------------------
// Mock getSession BEFORE importing the layout
// ------------------------------------------------------------
vi.mock('@/api/authentication/getSession');

import { getSession } from '@/api/authentication/getSession';
import { apiClientMock } from '@/test/setup';

// Strongly typed mock instance
const getSessionMock = getSession as MockedFunction<typeof getSession>;

describe('PublicLayout', () => {
  beforeEach(() => {
    apiClientMock.get.mockReset();
    getSessionMock.mockReset();
  });

  async function loadPage() {
    const mod = await import('@/app/(public)/layout');
    return mod.default;
  }

  it('always shows children', async () => {
    // Authenticated by default
    getSessionMock.mockResolvedValue({
      success: true,
      data: { isAuthenticated: true },
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByText('child')).toBeInTheDocument();
  });

  it('shows logout when authenticated', async () => {
    getSessionMock.mockResolvedValue({
      success: true,
      data: { isAuthenticated: true },
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByRole('button', { name: /logout/i })).toBeInTheDocument();
  });

  it('shows login when unauthenticated', async () => {
    getSessionMock.mockResolvedValue({
      success: true,
      data: { isAuthenticated: false },
    });

    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByRole('button', { name: /login/i })).toBeInTheDocument();
  });
});
