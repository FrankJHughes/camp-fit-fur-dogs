import { describe, it, expect, beforeEach, vi, MockedFunction } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// Mock module BEFORE importing the layout
vi.mock('@/api/authentication/getSession');

import { getSession } from '@/api/authentication/getSession';
import { apiClientMock } from '@/test/setup';

// Strongly-typed mock
const getSessionMock = getSession as MockedFunction<typeof getSession>;

describe('AuthenticatedLayout', () => {
  beforeEach(() => {
    apiClientMock.get.mockReset();
    getSessionMock.mockReset();
  });

  async function loadPage() {
    const mod = await import('@/app/(authenticated)/layout');
    return mod.default;
  }

  it('shows children when authenticated', async () => {
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
    expect(await screen.findByRole('button', { name: /logout/i })).toBeInTheDocument();
  });

  it('shows login message when unauthenticated', async () => {
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

    expect(await screen.findByText(/login to view/i)).toBeInTheDocument();
    expect(await screen.findByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  it('calls logout endpoint', async () => {
    getSessionMock.mockResolvedValue({
      success: true,
      data: { isAuthenticated: true },
    });

    apiClientMock.get.mockResolvedValue({ ok: true, data: {} });

    const Layout = await loadPage();
    const user = userEvent.setup();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    await user.click(await screen.findByRole('button', { name: /logout/i }));

    expect(apiClientMock.get).toHaveBeenCalledWith(
      `/auth/logout?return_url=${encodeURIComponent(window.location.href)}`
    );
  });
});
