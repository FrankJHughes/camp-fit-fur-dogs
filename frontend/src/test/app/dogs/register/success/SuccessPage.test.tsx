import { render, screen } from '@testing-library/react';
import SuccessPage from '@/app/dogs/register/success/page';

describe('Register Dog Success Page', () => {
  it('renders a success confirmation message', () => {
    render(<SuccessPage />);

    expect(
      screen.getByRole('heading', { name: /registration successful/i })
    ).toBeInTheDocument();
  });
});
