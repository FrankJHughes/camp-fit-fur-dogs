import { useEffect, useRef } from 'react';

export interface ConfirmDialogProps {
    isOpen: boolean;
    title: string;
    description: string;
    confirmLabel: string;
    onConfirm: () => void;
    onCancel: () => void;
}

export function ConfirmDialog({
    isOpen,
    title,
    description,
    confirmLabel,
    onConfirm,
    onCancel,
}: ConfirmDialogProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const cancelRef = useRef<HTMLButtonElement>(null);

    useEffect(() => {
        const dialog = dialogRef.current;
        if (!dialog) return;

        if (isOpen) {
            if (!dialog.open) dialog.showModal();
            cancelRef.current?.focus();
        } else {
            if (dialog.open) dialog.close();
        }
    }, [isOpen]);

    return (
        <dialog
            ref={dialogRef}
            onCancel={(e) => e.preventDefault()}
            onKeyDown={(e) => {
                if (e.key === 'Escape') {
                    e.preventDefault();
                    onCancel();
                }
            }}
        >
            <h2>{title}</h2>
            <p>{description}</p>
            <button onClick={onConfirm}>{confirmLabel}</button>
            <button ref={cancelRef} onClick={onCancel}>Cancel</button>
        </dialog>
    );
}
