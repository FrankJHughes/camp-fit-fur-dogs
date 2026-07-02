import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';

async function loadLayout() {
  const mod = await import('@/app/(public)/layout');
  return mod.default;
}

describe('(public)/layout', () => {
  it('always renders children', async () => {
    const Layout = await loadLayout();
    render(<Layout><div>child</div></Layout>);
    expect(screen.getByText('child')).toBeInTheDocument();
  });
});
