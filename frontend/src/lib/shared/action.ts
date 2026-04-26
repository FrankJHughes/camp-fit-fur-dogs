export interface Action {
    label: string;
    onClick: () => void;
    variant?: 'default' | 'destructive';
}
