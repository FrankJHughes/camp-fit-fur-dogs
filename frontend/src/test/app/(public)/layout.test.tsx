import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import PublicLayout from '@/app/(public)/layout';

describe('(public) layout', () => {
  it('renders header and children', () => {
    render(
      <PublicLayout>
        <div>child</div>
      </PublicLayout>
    );

    expect(screen.getByText('Camp Fit Fur Dogs')).toBeInTheDocument();
    expect(screen.getByText('child')).toBeInTheDocument();
  });
});
