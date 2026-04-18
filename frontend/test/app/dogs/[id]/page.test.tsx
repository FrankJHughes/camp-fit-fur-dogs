import { render, screen, waitFor } from '@testing-library/react';
import ViewDogProfilePage from '@/app/dogs/[id]/page';
import { getDogProfile } from '@/api/getDogProfile';

vi.mock('@/api/getDogProfile');
vi.mock('next/navigation', () => ({
  useParams: () => ({ id: 'abc-123' }),
}));

const mockedGetDogProfile = vi.mocked(getDogProfile);

const profile = {
  id: 'abc-123',
  ownerId: 'owner-1',
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('ViewDogProfilePage', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('shows a loading indicator while fetching', () => {
    mockedGetDogProfile.mockReturnValue(new Promise(() => {}));

    render(<ViewDogProfilePage />);

    expect(screen.getByText('Loading…')).toBeInTheDocument();
  });

  it('renders the dog profile on success', async () => {
    mockedGetDogProfile.mockResolvedValue({ success: true, profile });

    render(<ViewDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Buddy' })).toBeInTheDocument();
    });
  });

  it('shows a friendly message when the dog is not found', async () => {
    mockedGetDogProfile.mockResolvedValue({ success: false, notFound: true });

    render(<ViewDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByText('Dog not found.')).toBeInTheDocument();
    });
  });

  it('shows an error message on failure', async () => {
    mockedGetDogProfile.mockResolvedValue({
      success: false,
      notFound: false,
      error: 'An unexpected error occurred. Please try again.',
    });

    render(<ViewDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByText('An unexpected error occurred. Please try again.')).toBeInTheDocument();
    });
  });
});