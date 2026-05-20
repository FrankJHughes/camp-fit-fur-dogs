'use client';

import { CreateAccountForm } from '@/components/account/CreateAccountForm';
import { useCreateAccount } from '@/lib/account/useCreateAccount';

export default function CreateAccountPage() {
  const { command } = useCreateAccount();

  return <CreateAccountForm command={command} />;
}
