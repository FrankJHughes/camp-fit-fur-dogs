import { useState } from 'react';
import { removeDog } from '@/api/dogs/removeDog';
import { useConfirmDialog } from '@/lib/hooks/useConfirmDialog';
import type { ConfirmDialogProps } from '@/lib/components/ConfirmDialog';

export function useRemoveDog(
    id: string,
    name: string,
    push: (url: string) => void
) {
    const dialog = useConfirmDialog();
    const [error, setError] = useState<string | null>(null);

    const open = () => {
        if (!name) return;
        setError(null);
        dialog.open();
    };

    const dialogProps: ConfirmDialogProps = {
        isOpen: dialog.isOpen,
        title: `Remove ${name}?`,
        description: `This will permanently remove ${name} from your account. This cannot be undone.`,
        confirmLabel: 'Remove',
        onConfirm: async () => {
            const result = await removeDog(id);
            if (result.success) {
                push('/dogs');
            } else {
                setError(result.errors?.form ?? 'Something went wrong');
            }
            dialog.close();
        },
        onCancel: dialog.close,
    };

    return { open, dialogProps, error };
}

