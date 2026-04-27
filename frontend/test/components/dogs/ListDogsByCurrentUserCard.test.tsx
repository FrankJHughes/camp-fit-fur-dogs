import { render, screen } from '@testing-library/react';
import { ListDogsByCurrentUserCard } from '@/components/dogs/ListDogsByCurrentUserCard';

vi.mock('next/link', () => ({
  default: (props: { href: string; children: React.ReactNode }) => <a href={props.href}>{props.children}</a>,
}));

describe('ListDogsByCurrentUserCard', () => {
  it('renders each dog as a link with name and breed', () => {
    const dogs = [
      { id: '1', name: 'Biscuit', breed: 'Golden Retriever' },
      { id: '2', name: 'Maple', breed: 'Beagle' },
    ];

    render(<ListDogsByCurrentUserCard dogs={dogs} />);

    expect(screen.getByRole('heading', { name: 'My Dogs' })).toBeInTheDocument();

    const links = screen.getAllByRole('link');
    expect(links).toHaveLength(2);
    expect(links[0]).toHaveAttribute('href', '/dogs/1');
    expect(links[0]).toHaveTextContent(/Biscuit/);
    expect(links[0]).toHaveTextContent(/Golden Retriever/);
    expect(links[1]).toHaveAttribute('href', '/dogs/2');
    expect(links[1]).toHaveTextContent(/Maple/);
    expect(links[1]).toHaveTextContent(/Beagle/);
  });

  it('shows empty state when no dogs registered', () => {
    render(<ListDogsByCurrentUserCard dogs={[]} />);

    expect(screen.getByText('No dogs registered yet.')).toBeInTheDocument();
    expect(screen.queryAllByRole('link')).toHaveLength(0);
  });
});
