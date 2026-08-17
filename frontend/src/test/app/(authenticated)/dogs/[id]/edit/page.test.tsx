import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// Mock next/navigation
const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
  useParams: () => ({ id: 'dog-123' }),
  useRouter: () => ({ push: mockPush }),
}));

// Mock API functions
vi.mock('@/api/dogs/getDog');
vi.mock('@/api/dogs/editDog');

import { Dog } from '@/lib/dogs/dogModel';
import { getDog } from '@/api/dogs/getDog';
import { editDog } from '@/api/dogs/editDog';
import EditDogPage from '@/app/(authenticated)/dogs/[id]/edit/page';

// --- FIXED PROFILE DATA (sex must be 'Male' | 'Female') ---
const profileData: Dog = {
  id: 'dog-123',
  ownerId: 'owner-456',
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male', // MUST be 'Male' | 'Female'
};

describe('EditDogPage', () => {
  beforeEach(() => {
    vi.mocked(getDog).mockResolvedValue({
      success: true,
      data: profileData,
    });
    mockPush.mockClear();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('shows loading state initially', () => {
    render(<EditDogPage />);
    expect(screen.getByText(/loading/i)).toBeInTheDocument();
  });

  it('fetches the dog profile on mount', async () => {
    render(<EditDogPage />);

    await waitFor(() => {
      expect(getDog).toHaveBeenCalledWith('dog-123');
    });
  });

  it('renders the edit form with pre-populated data after loading', async () => {
    render(<EditDogPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
    });

    expect(screen.getByLabelText(/breed/i)).toHaveValue('Golden Retriever');
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('2023-06-15');
    expect(screen.getByLabelText(/sex/i)).toHaveValue('Male');
  });

  it('shows not found message when dog does not exist', async () => {
    vi.mocked(getDog).mockResolvedValue({
      success: false,
      notFound: true, // MUST be true, never false
    });

    render(<EditDogPage />);

    await waitFor(() => {
      expect(
        screen.getByRole('heading', { name: /couldn't find that dog/i })
      ).toBeInTheDocument();
    });
  });

  it('shows error message when fetching fails', async () => {
    vi.mocked(getDog).mockResolvedValue({
      success: false,
      error: 'An unexpected error occurred. Please try again.',
      // ❌ no notFound here — error and notFound are mutually exclusive
    });

    render(<EditDogPage />);

    await waitFor(() => {
      expect(
        screen.getByText('An unexpected error occurred. Please try again.')
      ).toBeInTheDocument();
    });
  });

  it('calls editDog and redirects on success', async () => {
    vi.mocked(editDog).mockResolvedValue({ success: true });
    const user = userEvent.setup();

    render(<EditDogPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
    });

    await user.clear(screen.getByLabelText(/name/i));
    await user.type(screen.getByLabelText(/name/i), 'Max');
    await user.click(screen.getByRole('button', { name: /save/i }));

    await waitFor(() => {
      expect(editDog).toHaveBeenCalledWith('dog-123', {
        name: 'Max',
        breed: 'Golden Retriever',
        dateOfBirth: '2023-06-15',
        sex: 'Male', // MUST match domain type
      });
    });

    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/dogs/dog-123');
    });
  }, 10000);

  it('displays server errors when editDog fails', async () => {
    vi.mocked(editDog).mockResolvedValue({
      success: false,
      errors: { name: 'Name is already taken' },
    });
    const user = userEvent.setup();

    render(<EditDogPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
    });

    await user.click(screen.getByRole('button', { name: /save/i }));

    await waitFor(() => {
      expect(screen.getByText('Name is already taken')).toBeInTheDocument();
    });
  }, 10000);
});
