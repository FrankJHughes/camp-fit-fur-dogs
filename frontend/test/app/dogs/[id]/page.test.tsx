import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi, beforeAll, beforeEach } from 'vitest';
import GetDogProfilePage from '@/app/dogs/[id]/page';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { removeDog } from '@/api/dogs/removeDog';

vi.mock('@/api/dogs/getDogProfile');
vi.mock('@/api/dogs/removeDog');

const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
    useParams: () => ({ id: 'abc-123' }),
    useRouter: () => ({ push: mockPush }),
}));

beforeAll(() => {
    HTMLDialogElement.prototype.showModal ??= vi.fn(function (this: HTMLDialogElement) {
        this.setAttribute('open', '');
    });
    HTMLDialogElement.prototype.close ??= vi.fn(function (this: HTMLDialogElement) {
        this.removeAttribute('open');
    });
});

const profile = {
    id: 'abc-123',
    name: 'Buddy',
    breed: 'Golden Retriever',
    dateOfBirth: '2023-06-15',
    sex: 'Male',
    ownerId: 'owner-1',
};

describe('GetDogProfilePage', () => {
    beforeEach(() => {
        vi.mocked(getDogProfile).mockReset();
        vi.mocked(removeDog).mockReset();
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

    it('renders an Edit button that navigates to the edit route', async () => {
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

    it('shows the confirm dialog when the Remove button is clicked', async () => {
        const user = userEvent.setup();
        vi.mocked(getDogProfile).mockResolvedValue({ success: true, data: profile });

        render(<GetDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByRole('button', { name: /remove/i })).toBeDefined();
        });

        await user.click(screen.getByRole('button', { name: /remove/i }));

        expect(screen.getByRole('dialog')).toBeInTheDocument();
        expect(screen.getByRole('heading', { name: /remove buddy/i })).toBeInTheDocument();
    });

    it('calls removeDog and navigates to /dogs when confirm is clicked', async () => {
        const user = userEvent.setup();
        vi.mocked(getDogProfile).mockResolvedValue({ success: true, data: profile });
        vi.mocked(removeDog).mockResolvedValue({ success: true });

        render(<GetDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByRole('button', { name: /remove/i })).toBeDefined();
        });

        await user.click(screen.getByRole('button', { name: /remove/i }));

        const dialog = screen.getByRole('dialog');
        await user.click(within(dialog).getByRole('button', { name: 'Remove' }));

        await waitFor(() => {
            expect(removeDog).toHaveBeenCalledWith('abc-123');
            expect(mockPush).toHaveBeenCalledWith('/dogs');
        });
    });

    it('closes the confirm dialog when cancel is clicked', async () => {
        const user = userEvent.setup();
        vi.mocked(getDogProfile).mockResolvedValue({ success: true, data: profile });

        render(<GetDogProfilePage />);

        await waitFor(() => {
            expect(screen.getByRole('button', { name: /remove/i })).toBeDefined();
        });

        await user.click(screen.getByRole('button', { name: /remove/i }));
        expect(screen.getByRole('dialog')).toBeInTheDocument();

        await user.click(screen.getByRole('button', { name: /cancel/i }));
        expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
    });
});
