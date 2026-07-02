import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';

describe('PublicLayout (integration)', () => {
  async function loadPage() {
    const mod = await import('@/app/(public)/layout');
    return mod.default;
  }

  it('always shows children', async () => {
    const Layout = await loadPage();

    render(
      <Layout>
        <div>child</div>
      </Layout>
    );

    expect(await screen.findByText('child')).toBeInTheDocument();
  });
});
