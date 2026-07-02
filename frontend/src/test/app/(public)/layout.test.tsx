import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';

describe('(public) layout', () => {
  async function renderLayout() {
    const { default: Layout } = await import('@/app/(public)/layout');
    render(
      <Layout>
        <div>child</div>
      </Layout>
    );
  }

  it('always shows children', async () => {
    await renderLayout();
    expect(await screen.findByText('child')).toBeInTheDocument();
  });
});
