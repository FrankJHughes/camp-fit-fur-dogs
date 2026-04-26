import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ActionsCard } from '@/components/shared/ActionsCard';
import type { Action } from '@/lib/shared/action';

describe('ActionsCard', () => {
    it('renders a button for each action', () => {
        const actions: Action[] = [
            { label: 'Edit', onClick: vi.fn() },
            { label: 'Remove', onClick: vi.fn() },
        ];

        render(<ActionsCard actions={actions} />);

        expect(screen.getByRole('button', { name: 'Edit' })).toBeDefined();
        expect(screen.getByRole('button', { name: 'Remove' })).toBeDefined();
    });

    it('calls onClick when a button is clicked', async () => {
        const user = userEvent.setup();
        const onClick = vi.fn();
        const actions: Action[] = [{ label: 'Do Thing', onClick }];

        render(<ActionsCard actions={actions} />);
        await user.click(screen.getByRole('button', { name: 'Do Thing' }));

        expect(onClick).toHaveBeenCalledOnce();
    });

    it('renders nothing when actions array is empty', () => {
        const { container } = render(<ActionsCard actions={[]} />);

        expect(container.innerHTML).toBe('');
    });

    it('renders a destructive action with data-variant="destructive"', () => {
        const actions: Action[] = [
            { label: 'Remove', onClick: vi.fn(), variant: 'destructive' },
        ];

        render(<ActionsCard actions={actions} />);

        expect(screen.getByRole('button', { name: 'Remove' })).toHaveAttribute(
            'data-variant',
            'destructive',
        );
    });

    it('renders a default action without data-variant', () => {
        const actions: Action[] = [
            { label: 'Edit', onClick: vi.fn() },
        ];

        render(<ActionsCard actions={actions} />);

        expect(screen.getByRole('button', { name: 'Edit' })).not.toHaveAttribute('data-variant');
    });
});
