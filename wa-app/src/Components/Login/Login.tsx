import './Login.scss';

import { FormEvent, useId, useRef, useState } from 'react';
import axios from 'axios';

function App(): JSX.Element {
	const emailId = useId();
	const passwordId = useId();
	const emailRef = useRef<HTMLInputElement>(null);
	const passwordRef = useRef<HTMLInputElement>(null);

	const onLoginSubmit = (e: FormEvent<HTMLFormElement>) => {
		const email = emailRef.current?.value;
		const password = passwordRef.current?.value;
		if (email == null || email.length === 0) {
			return;
		} else if (password == null || password.length === 0) {
			return;
		}

		axios.post('http://localhost:5000/login', {
			email: email
		}).then(res => {
			console.log(res.data);
		});

		e.preventDefault();
	};

	return (
		<div className='login-container'>
			<form className='login-form' onSubmit={onLoginSubmit}>
				<label htmlFor={emailId}>E-mail</label>
				<input type='email' id={emailId} ref={emailRef} />

				<label htmlFor={passwordId}>Password</label>
				<input type='password' id={passwordId} ref={passwordRef} />

				<button type='submit'>Submit</button>
			</form>
			<h2 className='login-or'>OR</h2>
			<a href='#' className='login-google'>Login with Google</a>
		</div>
	);
}

export default App;
