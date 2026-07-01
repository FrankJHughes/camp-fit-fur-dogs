import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import AuthenticatedLayout from '@/app/(authenticated)/layout';

describe('(authenticated) layout', () => {
  it('renders children', () => {
    render(
      <AuthenticatedLayout>
        <div>child</div>
      </AuthenticatedLayout>
    );

    expect(screen.getByText('child')).toBeInTheDocument();
  });
});
