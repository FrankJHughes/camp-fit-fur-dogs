import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import Home from '@/app/(public)/page';

describe('Home page', () => {
  it('renders the welcome message', async () => {
    render(<Home />);

    expect(
      screen.getByText('Welcome home.')
    ).toBeInTheDocument();
  });
});
