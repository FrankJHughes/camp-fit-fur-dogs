import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import LoggedOutPage from '@/app/(public)/logged-out/page';

describe('LoggedOutPage', () => {
  it('renders logged out message', () => {
    render(<LoggedOutPage />);

    expect(screen.getByText("You’ve been logged out")).toBeInTheDocument();
    expect(screen.getByText('Return to Login')).toBeInTheDocument();
  });
});
