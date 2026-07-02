import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import Home from '@/app/(public)/page';

// Mock useApiQuery BEFORE importing Home
const mockUseApiQuery = vi.fn();

vi.mock('@/lib/hooks/useApiQuery', () => ({
  useApiQuery: (...args: any[]) => mockUseApiQuery(...args),
}));

describe('Home page', () => {
  beforeEach(() => {
    mockUseApiQuery.mockReset();
  });

  it('renders the API Health heading', async () => {
    mockUseApiQuery.mockReturnValue({
      status: 'success',
      data: { status: 'Healthy' },
    });

    render(<Home />);

    expect(
      screen.getByRole('heading', { level: 1, name: /api health status/i })
    ).toBeInTheDocument();
  });
});
