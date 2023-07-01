import './Register.scss';

import { FormEvent, useEffect, useId, useRef, useState } from 'react';
import axios from 'axios';

export default function Register(): JSX.Element {
	const usernameId = useId();
	const emailId = useId();
	const passwordId = useId();
	const repeatPasswordId = useId();

	const [username, setUsername] = useState('');
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [passwordRepeat, setPasswordRepeat] = useState('');

	useEffect(() => {
		const delayDebounceFn = setTimeout(() => {
			axios.post('https://localhost:5000/AccountTaken', {
				username: 'abc',
				email: 'email'
			}).then(res => {
				console.log(res.data);
			});
		}, 1500);

		return () => clearTimeout(delayDebounceFn);
	}, [username]);

	useEffect(() => {
		const delayDebounceFn = setTimeout(() => {
			// TODO: Query GraphQL if this username already exists
		}, 1500);

		return () => clearTimeout(delayDebounceFn);
	}, [email]);

	const onRegisterSubmit = (e: FormEvent<HTMLFormElement>) => {
		e.preventDefault();
	};

	return (
		<div className='register-container'>
			<form className='register-form' onSubmit={onRegisterSubmit}>
				<label className='register-form__label--ok' htmlFor={usernameId}>Username</label>
				<input type='text' className='register-form__input--ok' id={usernameId} onChange={(e) => setUsername(e.target.value)} />

				<label htmlFor={emailId}>E-mail</label>
				<input type='email' id={emailId} onChange={(e) => setEmail(e.target.value)} />

				<label htmlFor={passwordId}>Password</label>
				<input type='password' id={passwordId} onChange={(e) => setPassword(e.target.value)} />

				<label htmlFor={repeatPasswordId}>Repeat password</label>
				<input type='password' id={repeatPasswordId} onChange={(e) => setPasswordRepeat(e.target.value)} />

				<button type='submit'>Submit</button>
			</form>
		</div>
	);
}
