import { describe, it, expect, vi, MockedFunction } from 'vitest';
import { render, screen } from '@testing-library/react';

// Mock BEFORE importing the layout
vi.mock('@/api/authentication/getSession');

import { getSession } from '@/api/authentication/getSession';
const getSessionMock = getSession as MockedFunction<typeof getSession>;

import PublicLayout from '@/app/(public)/layout';

describe('(public) layout', () => {
  it('renders header and children', () => {
    getSessionMock.mockResolvedValue({
      success: true,
      data: { isAuthenticated: true },
    });

    render(
      <PublicLayout>
        <div>child</div>
      </PublicLayout>
    );

    expect(screen.getByText('Camp Fit Fur Dogs')).toBeInTheDocument();
    expect(screen.getByText('child')).toBeInTheDocument();
  });
});
