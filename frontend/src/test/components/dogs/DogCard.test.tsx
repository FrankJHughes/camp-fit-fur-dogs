import { render, screen } from '@testing-library/react';
import { DogCard } from '@/components/dogs/DogCard';

const profile = {
  id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
  ownerId: 'owner-1234',
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('DogCard', () => {
  it('renders the dog name as a heading', () => {
    render(<DogCard profile={profile} />);
    expect(screen.getByRole('heading', { name: 'Buddy' })).toBeInTheDocument();
  });

  it('renders the breed', () => {
    render(<DogCard profile={profile} />);
    expect(screen.getByText('Golden Retriever')).toBeInTheDocument();
  });

  it('renders the date of birth', () => {
    render(<DogCard profile={profile} />);
    expect(screen.getByText('June 15, 2023')).toBeInTheDocument();
  });

  it('renders the sex', () => {
    render(<DogCard profile={profile} />);
    expect(screen.getByText('Male')).toBeInTheDocument();
  });

  it('does not expose the owner ID', () => {
    render(<DogCard profile={profile} />);
    expect(screen.queryByText('owner-1234')).not.toBeInTheDocument();
  });

});
