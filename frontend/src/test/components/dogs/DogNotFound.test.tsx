import { render, screen } from '@testing-library/react';
import { DogNotFound } from '@/components/dogs/DogNotFound';

describe('DogNotFound', () => {
  it('renders a friendly heading about the missing dog', () => {
    render(<DogNotFound />);
    expect(
      screen.getByRole('heading', { name: /couldn't find that dog/i }),
    ).toBeInTheDocument();
  });

  it('renders an explanatory message', () => {
    render(<DogNotFound />);
    expect(
      screen.getByText(/may have been removed/i),
    ).toBeInTheDocument();
  });

  it('links to the dog list', () => {
    render(<DogNotFound />);
    const link = screen.getByRole('link', { name: /view your dogs/i });
    expect(link).toHaveAttribute('href', '/dogs');
  });

  it('links to register a new dog', () => {
    render(<DogNotFound />);
    const link = screen.getByRole('link', { name: /register a new dog/i });
    expect(link).toHaveAttribute('href', '/dogs/register');
  });
});