import { describe, it, expect, vi } from 'vitest';
import { getDogProfileActions } from '@/lib/dogs/dogProfileActions';

describe('getDogProfileActions', () => {
    it('returns an array containing an Edit action that pushes to the edit route', () => {
        const push = vi.fn();

        const actions = getDogProfileActions('abc-123', push);

        expect(actions).toHaveLength(1);

        const editAction = actions.find(a => a.label === 'Edit');
        expect(editAction).toBeDefined();

        editAction!.onClick();
        expect(push).toHaveBeenCalledWith('/dogs/abc-123/edit');
    });
});
