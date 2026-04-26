import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import DogProfileActionsCard from '@/components/dogs/DogProfileActionsCard';
import type { DogProfileAction } from '@/lib/dogs/dogProfileActions';

describe('DogProfileActionsCard', () => {
    it('renders a button for each action and calls the handler on click', async () => {
        const user = userEvent.setup();
        const handleEdit = vi.fn();
        const actions: DogProfileAction[] = [
            { label: 'Edit', onClick: handleEdit },
        ];

        render(<DogProfileActionsCard actions={actions} />);

        await user.click(screen.getByRole('button', { name: /edit/i }));

        expect(handleEdit).toHaveBeenCalledOnce();
    });
});
