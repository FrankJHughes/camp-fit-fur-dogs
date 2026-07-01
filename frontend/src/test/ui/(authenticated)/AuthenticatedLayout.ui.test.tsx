import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// mock API client BEFORE importing the page
import { apiClientMock } from '@/test/setup';
import { pushMock } from '@/test/helpers/mockRouter';

describe('Logout (integration)', () => {
  beforeEach(() => {
    vi.resetModules();
    apiClientMock.post.mockReset();
    pushMock.mockReset();
  });

  async function loadPage() {
    // authenticated layout or page containing logout action
    const mod = await import('@/app/(authenticated)/layout');
    return mod.default;
  }

  it('logs out and allows the backend redirect', async () => {
    apiClientMock.post.mockResolvedValue({ ok: true });

    const Layout = await loadPage();
    const user = userEvent.setup();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    await user.click(screen.getByRole('button', { name: /logout/i }));

    expect(apiClientMock.post).toHaveBeenCalledWith('/auth/logout', {});
  });

  it('shows error and does not navigate on failure', async () => {
    apiClientMock.post.mockRejectedValue(new Error('boom'));

    const Layout = await loadPage();
    const user = userEvent.setup();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    await user.click(screen.getByRole('button', { name: /logout/i }));

    expect(await screen.findByText('boom')).toBeInTheDocument();
    expect(pushMock).not.toHaveBeenCalled();
  });
});
