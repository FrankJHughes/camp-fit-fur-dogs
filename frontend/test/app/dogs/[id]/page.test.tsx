import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import GetDogProfilePage from '@/app/dogs/[id]/page';
import { getDogProfile } from '@/api/dogs/getDogProfile';

vi.mock('@/api/dogs/getDogProfile');

const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
  useParams: () => ({ id: 'abc-123' }),
  useRouter: () => ({ push: mockPush }),
}));

const profile = {
  id: 'abc-123',
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
  ownerId: 'owner-1',
};

describe('ViewDogProfilePage', () => {
  beforeEach(() => {
    vi.mocked(getDogProfile).mockReset();
    mockPush.mockClear();
  });

  it('shows loading state initially', () => {
    vi.mocked(getDogProfile).mockReturnValue(new Promise(() => { }));

    render(<GetDogProfilePage />);

    expect(screen.getByText('Loading…')).toBeDefined();
  });

  it('renders the dog profile card with the fetched profile', async () => {
    vi.mocked(getDogProfile).mockResolvedValue({ success: true, data: profile });

    render(<GetDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByText('Buddy')).toBeDefined();
    });
  });

  it('renders the actions card with an Edit button that navigates to the edit route', async () => {
    const user = userEvent.setup();
    vi.mocked(getDogProfile).mockResolvedValue({ success: true, data: profile });

    render(<GetDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /edit/i })).toBeDefined();
    });

    await user.click(screen.getByRole('button', { name: /edit/i }));

    expect(mockPush).toHaveBeenCalledWith('/dogs/abc-123/edit');
  });

  it('shows not-found message when the dog does not exist', async () => {
    vi.mocked(getDogProfile).mockResolvedValue({ success: false, notFound: true });

    render(<GetDogProfilePage />);

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /couldn't find that dog/i })).toBeInTheDocument();
    });
  });
});
