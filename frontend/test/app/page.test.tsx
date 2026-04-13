import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import Home from '@/app/page';

describe('Home page', () => {
  it('renders the heading', () => {
    vi.stubGlobal('fetch', vi.fn(() => new Promise(() => {})));

    render(<Home />);

    expect(
      screen.getByRole('heading', { level: 1, name: /camp fit fur dogs/i })
    ).toBeInTheDocument();

    vi.restoreAllMocks();
  });
});
