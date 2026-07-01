export interface Action {
  label: string;
  onClick: () => void;
  variant?: 'default' | string;
  disabled?: boolean;
}
