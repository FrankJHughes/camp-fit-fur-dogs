import { render, screen } from '@testing-library/react';
import { NotFound } from '@/lib/components/NotFound';

describe('NotFound', () => {
  const defaultProps = {
    heading: "We couldn't find that dog",
    message: 'They may have been removed, or the link might be outdated.',
    links: [
      { label: 'View your dogs', href: '/dogs' },
      { label: 'Register a new dog', href: '/dogs/register' },
    ],
  };

  it('renders the heading', () => {
    render(<NotFound {...defaultProps} />);
    expect(
      screen.getByRole('heading', { name: /we couldn't find that dog/i }),
    ).toBeInTheDocument();
  });

  it('renders the message', () => {
    render(<NotFound {...defaultProps} />);
    expect(
      screen.getByText(/they may have been removed/i),
    ).toBeInTheDocument();
  });

  it('renders each link with the correct label', () => {
    render(<NotFound {...defaultProps} />);
    expect(screen.getByRole('link', { name: /view your dogs/i })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /register a new dog/i })).toBeInTheDocument();
  });

  it('renders each link with the correct href', () => {
    render(<NotFound {...defaultProps} />);
    expect(screen.getByRole('link', { name: /view your dogs/i })).toHaveAttribute('href', '/dogs');
    expect(screen.getByRole('link', { name: /register a new dog/i })).toHaveAttribute('href', '/dogs/register');
  });

  it('renders without links when none are provided', () => {
    render(<NotFound {...defaultProps} links={[]} />);
    expect(screen.getByRole('heading', { name: /we couldn't find that dog/i })).toBeInTheDocument();
    expect(screen.queryAllByRole('link')).toHaveLength(0);
  });

  it('works for any aggregate, not just dogs', () => {
    render(
      <NotFound
        heading="We couldn't find that booking"
        message="It may have been cancelled."
        links={[{ label: 'View bookings', href: '/bookings' }]}
      />,
    );
    expect(screen.getByRole('heading', { name: /booking/i })).toBeInTheDocument();
    expect(screen.getByText(/cancelled/i)).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /view bookings/i })).toHaveAttribute('href', '/bookings');
  });
});
