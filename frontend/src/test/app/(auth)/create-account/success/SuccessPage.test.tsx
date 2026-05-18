import { render, screen } from '@testing-library/react';
import SuccessPage from '@/app/(auth)/create-account/success/page';

describe('Create Account Success Page', () => {
  it('renders a success confirmation message', () => {
    render(<SuccessPage />);

    expect(
      screen.getByRole('heading', { name: /creation successful/i })
    ).toBeInTheDocument();
  });
});
