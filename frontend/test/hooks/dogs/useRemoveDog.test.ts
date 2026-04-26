import { renderHook, act } from '@testing-library/react';
import { useRemoveDog } from '@/hooks/dogs/useRemoveDog';
import { removeDog } from '@/api/dogs/removeDog';

vi.mock('@/api/dogs/removeDog');

describe('useRemoveDog', () => {
    beforeEach(() => {
        vi.mocked(removeDog).mockReset();
    });

    it('returns dialogProps with isOpen false initially', () => {
        const push = vi.fn();
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        expect(result.current.dialogProps.isOpen).toBe(false);
    });

    it('opens the dialog when open is called', () => {
        const push = vi.fn();
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        act(() => result.current.open());

        expect(result.current.dialogProps.isOpen).toBe(true);
    });

    it('closes the dialog when onCancel is called', () => {
        const push = vi.fn();
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        act(() => result.current.open());
        expect(result.current.dialogProps.isOpen).toBe(true);

        act(() => result.current.dialogProps.onCancel());
        expect(result.current.dialogProps.isOpen).toBe(false);
    });

    it('calls removeDog and navigates to /dogs on confirm success', async () => {
        const push = vi.fn();
        vi.mocked(removeDog).mockResolvedValue({ success: true });
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        act(() => result.current.open());
        await act(async () => result.current.dialogProps.onConfirm());

        expect(removeDog).toHaveBeenCalledWith('abc-123');
        expect(push).toHaveBeenCalledWith('/dogs');
        expect(result.current.dialogProps.isOpen).toBe(false);
    });

    it('does not navigate when removeDog fails', async () => {
        const push = vi.fn();
        vi.mocked(removeDog).mockResolvedValue({ success: false, errors: { form: 'Error' } });
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        act(() => result.current.open());
        await act(async () => result.current.dialogProps.onConfirm());

        expect(removeDog).toHaveBeenCalledWith('abc-123');
        expect(push).not.toHaveBeenCalled();
        expect(result.current.dialogProps.isOpen).toBe(false);
    });

    it('returns dialog title and description containing the dog name', () => {
        const push = vi.fn();
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        expect(result.current.dialogProps.title).toBe('Remove Buddy?');
        expect(result.current.dialogProps.description).toContain('Buddy');
    });

    it('returns confirmLabel as "Remove"', () => {
        const push = vi.fn();
        const { result } = renderHook(() => useRemoveDog('abc-123', 'Buddy', push));

        expect(result.current.dialogProps.confirmLabel).toBe('Remove');
    });
});
