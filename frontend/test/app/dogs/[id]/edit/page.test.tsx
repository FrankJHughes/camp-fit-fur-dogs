import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// Mock next/navigation
const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
    useParams: () => ({ id: 'dog-123' }),
    useRouter: () => ({ push: mockPush }),
}));

// Mock API functions
vi.mock('@/api/dogs/getDogProfile');
vi.mock('@/api/dogs/editDogProfile');

import { getDogProfile } from '@/api/dogs/getDogProfile';
import { editDogProfile } from '@/api/dogs/editDogProfile';
import EditDogProfilePage from '@/app/dogs/[id]/edit/page';

const profileData = {
    id: 'dog-123',
    ownerId: 'owner-456',
    name: 'Buddy',
    breed: 'Golden Retriever',
    dateOfBirth: '2023-06-15',
    sex: 'Male',
};

describe('EditDogProfilePage', () => {
    beforeEach(() => {
        vi.mocked(getDogProfile).mockResolvedValue({
            success: true,
            profile: profileData,
        });
        mockPush.mockClear();
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('shows loading state initially', () => {
        render(<EditDogProfilePage />);
        expect(screen.getByText(/loading/i)).toBeInTheDocument();
    });

    it('fetches the dog profile on mount', async () => {
        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(getDogProfile).toHaveBeenCalledWith('dog-123');
        });
    });

    it('renders the edit form with pre-populated data after loading', async () => {
        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
        });

        expect(screen.getByLabelText(/breed/i)).toHaveValue(
            'Golden Retriever'
        );
        expect(screen.getByLabelText(/date of birth/i)).toHaveValue(
            '2023-06-15'
        );
        expect(screen.getByLabelText(/sex/i)).toHaveValue('Male');
    });

    it('shows not found message when dog does not exist', async () => {
        vi.mocked(getDogProfile).mockResolvedValue({
            success: false,
            notFound: true,
        });

        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByText(/not found/i)).toBeInTheDocument();
        });
    });

    it('shows error message when fetching fails', async () => {
        vi.mocked(getDogProfile).mockResolvedValue({
            success: false,
            notFound: false,
            error: 'An unexpected error occurred. Please try again.',
        });

        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(
                screen.getByText(
                    'An unexpected error occurred. Please try again.'
                )
            ).toBeInTheDocument();
        });
    });

    it('calls editDogProfile and redirects on success', async () => {
        vi.mocked(editDogProfile).mockResolvedValue({ success: true });
        const user = userEvent.setup();

        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
        });

        await user.clear(screen.getByLabelText(/name/i));
        await user.type(screen.getByLabelText(/name/i), 'Max');
        await user.click(screen.getByRole('button', { name: /save/i }));

        await waitFor(() => {
            expect(editDogProfile).toHaveBeenCalledWith('dog-123', {
                name: 'Max',
                breed: 'Golden Retriever',
                dateOfBirth: '2023-06-15',
                sex: 'Male',
            });
        });

        await waitFor(() => {
            expect(mockPush).toHaveBeenCalledWith('/dogs/dog-123');
        });
    }, 10000);

    it('displays server errors when editDogProfile fails', async () => {
        vi.mocked(editDogProfile).mockResolvedValue({
            success: false,
            errors: { name: 'Name is already taken' },
        });
        const user = userEvent.setup();

        render(<EditDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByLabelText(/name/i)).toHaveValue('Buddy');
        });

        await user.click(screen.getByRole('button', { name: /save/i }));

        await waitFor(() => {
            expect(
                screen.getByText('Name is already taken')
            ).toBeInTheDocument();
        });
    }, 10000);
});
