import { render, screen, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import Home from '@/app/(public)/page';
import { getHealth } from '@/api/health/getHealth';

vi.mock('@/api/health/getHealth');

describe('Home page', () => {
  beforeEach(() => {
    vi.mocked(getHealth).mockReset();
  });

  it('renders the heading', async () => {
    vi.mocked(getHealth).mockResolvedValue({
      success: true,
      data: { status: 'Healthy' }
    });

    render(<Home />);

    await waitFor(() => {
      expect(
        screen.getByRole('heading', { level: 1, name: /camp fit fur dogs/i })
      ).toBeInTheDocument();
    });
  });
});
