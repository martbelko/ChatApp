import './Register.scss';

import { FormEvent, useEffect, useId, useState } from 'react';
import axios from 'axios';

import checkIcon from './../../assets/icons/check.svg';
import crossIcon from './../../assets/icons/cross.svg';

interface AccountTakenPayload {
	usernameTaken: boolean;
	emailTaken: boolean;
}

enum DataState {
	None, Loading, Ok, NotOk, Invalid
}

export default function Register(): JSX.Element {
	const usernameId = useId();
	const emailId = useId();
	const passwordId = useId();
	const repeatPasswordId = useId();

	const [username, setUsername] = useState('');
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [passwordRepeat, setPasswordRepeat] = useState('');

	const [usernameDataState, setUsernameDataState] = useState(DataState.None);
	const [emailDataState, setEmailDataState] = useState(DataState.None);

	const [submitEnabled, setSubmitEnabled] = useState(false);

	const getClassModifierFromDataState = (dataState: DataState) => {
		switch (dataState) {
		case DataState.Loading:
			return 'loading';
		case DataState.Ok:
			return 'ok';
		case DataState.NotOk:
		case DataState.Invalid:
			return 'notok';
		}

		return '';
	};

	const checkSubmitEnable = () => {
		if (usernameDataState === DataState.Ok && emailDataState === DataState.Ok && password === passwordRepeat) {
			setSubmitEnabled(true);
		} else {
			setSubmitEnabled(false);
		}
	};

	useEffect(() => {
		if (username.length === 0) {
			setUsernameDataState(DataState.None);
		} else {
			setUsernameDataState(DataState.Loading);
		}

		const delayDebounceFn = setTimeout(() => {
			if (username.length === 0) {
				setUsernameDataState(DataState.None);
				return;
			}

			axios.post('https://localhost:5000/AccountTaken', {
				username: username
			}).then(res => {
				if (res != null && res.data != null) {
					const payload = res.data as AccountTakenPayload;
					setUsernameDataState(payload.usernameTaken ? DataState.NotOk : DataState.Ok);
					checkSubmitEnable();
				}
			});
		}, 1500);

		return () => clearTimeout(delayDebounceFn);
	}, [username]);

	useEffect(() => {
		if (email.length === 0) {
			setEmailDataState(DataState.None);
		} else {
			setEmailDataState(DataState.Loading);
		}

		const delayDebounceFn = setTimeout(() => {
			if (email.length === 0) {
				setEmailDataState(DataState.None);
				return;
			}

			// TODO: Use email regex
			if (email.length < 3 || !email.includes('@')) {
				setEmailDataState(DataState.Invalid);
				return;
			}

			axios.post('https://localhost:5000/AccountTaken', {
				email: email
			}).then(res => {
				if (res != null && res.data != null) {
					const payload = res.data as AccountTakenPayload;
					setEmailDataState(payload.emailTaken ? DataState.NotOk : DataState.Ok);
					checkSubmitEnable();
				}
			});
		}, 1500);

		return () => clearTimeout(delayDebounceFn);
	}, [email]);

	useEffect(() => {
		if (password.length > 0 && passwordRepeat.length > 0) {
			checkSubmitEnable();
		}
	}, [password, passwordRepeat]);

	const onRegisterSubmit = (e: FormEvent<HTMLFormElement>) => {
		e.preventDefault();
	};

	const usernameClassModifier = usernameDataState === DataState.None ? '' : getClassModifierFromDataState(usernameDataState);
	const usernameInputClass = usernameDataState === DataState.None ? '' : `register-form__input--${usernameClassModifier}`;
	const usernameLabelClass = usernameDataState === DataState.None ? '' : `register-form__label--${usernameClassModifier}`;

	const emailClassModifier = emailDataState === DataState.None ? '' : getClassModifierFromDataState(emailDataState);
	const emailInputClass = emailDataState === DataState.None ? '' : `register-form__input--${emailClassModifier}`;
	const emailLabelClass = emailDataState === DataState.None ? '' : `register-form__label--${emailClassModifier}`;

	const getUsernameDescription = () => {
		switch (usernameDataState) {
		case DataState.Ok:
			return <img src={checkIcon} alt='check' />;
		case DataState.NotOk:
			return (
				<>
					<img src={crossIcon} alt='cross' />Invalid
				</>);
		case DataState.Loading:
			return 'Loading';
		case DataState.Invalid:
			return 'Invalid';
		}

		return '';
	};

	return (
		<div className='register-container'>
			<form className='register-form' onSubmit={onRegisterSubmit}>
				<div className='register-form__element'>
					<label className={usernameLabelClass} htmlFor={usernameId}>Username</label>
					<div className='register-form__element__input-container'>
						<input
							minLength={4}
							maxLength={30}
							type='text'
							className={usernameInputClass}
							id={usernameId}
							onChange={(e) => setUsername(e.target.value)}
						/>
						<span className='register-form__description'>
							{getUsernameDescription()}
						</span>
					</div>
				</div>

				<div className='register-form__element'>
					<label className={emailLabelClass} htmlFor={emailId}>E-mail</label>
					<div className='register-form__element__input-container'>
						<input type='email' minLength={3} maxLength={320} className={emailInputClass} id={emailId} onChange={(e) => setEmail(e.target.value)} />
						<span className='register-form__description'>
							{''}
						</span>
					</div>
				</div>

				<div className='register-form__element'>
					<label htmlFor={passwordId}>Password</label>
					<div className='register-form__element__input-container'>
						<input type='password' minLength={8} maxLength={128} id={passwordId} onChange={(e) => setPassword(e.target.value)} />
						<span className='register-form__description'>
							{''}
						</span>
					</div>
				</div>

				<div className='register-form__element'>
					<label htmlFor={repeatPasswordId}>Repeat password</label>
					<div className='register-form__element__input-container'>
						<input type='password' minLength={8} maxLength={128} id={repeatPasswordId} onChange={(e) => setPasswordRepeat(e.target.value)} />
						<span className='register-form__description'>
							{''}
						</span>
					</div>
				</div>

				<button disabled={!submitEnabled} type='submit'>Submit</button>
			</form>
		</div>
	);
}
