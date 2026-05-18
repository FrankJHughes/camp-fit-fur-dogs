import { renderHook, act } from '@testing-library/react';
import { useConfirmDialog } from '@/lib/hooks/useConfirmDialog';

describe('useConfirmDialog', () => {
    it('starts closed', () => {
        const { result } = renderHook(() => useConfirmDialog());

        expect(result.current.isOpen).toBe(false);
    });

    it('opens when open is called', () => {
        const { result } = renderHook(() => useConfirmDialog());

        act(() => result.current.open());

        expect(result.current.isOpen).toBe(true);
    });

    it('closes when close is called', () => {
        const { result } = renderHook(() => useConfirmDialog());

        act(() => result.current.open());
        act(() => result.current.close());

        expect(result.current.isOpen).toBe(false);
    });
});
