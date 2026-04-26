import { useState } from 'react';
import { removeDog } from '@/api/dogs/removeDog';
import type { ConfirmDialogProps } from '@/components/shared/ConfirmDialog';

export function useRemoveDog(
    id: string,
    name: string,
    push: (url: string) => void
) {
    const [isOpen, setIsOpen] = useState(false);

    const dialogProps: ConfirmDialogProps = {
        isOpen,
        title: `Remove ${name}?`,
        description: `This will permanently remove ${name} from your account. This cannot be undone.`,
        confirmLabel: 'Remove',
        onConfirm: async () => {
            const result = await removeDog(id);
            if (result.success) {
                push('/dogs');
            }
            setIsOpen(false);
        },
        onCancel: () => setIsOpen(false),
    };

    return { open: () => setIsOpen(true), dialogProps };
}
