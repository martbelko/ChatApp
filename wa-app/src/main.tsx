import './main.scss';

import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { ApolloClient, ApolloProvider, InMemoryCache, from, HttpLink } from '@apollo/client';
import { onError } from '@apollo/client/link/error';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from './Components/Login/Login';
import Register from './Components/Register/Register';

const errorLink = onError((errors) => {
	if (errors.graphQLErrors) {
		errors.graphQLErrors.map((graphqlError) => {
			alert(`GraphQL error ${graphqlError.message}`);
		});
	}
});

const link = from([
	errorLink,
	new HttpLink({ uri: 'http://localhost:5000/graphql/' }),
]);

const client = new ApolloClient({
	cache: new InMemoryCache(),
	link: link
});

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
	<React.StrictMode>
		<ApolloProvider client={client}>
			<BrowserRouter>
				<Routes>
					<Route path='/' element={<App />} />
					<Route path='login' element={<Login />} />
					<Route path='register' element={<Register />} />
				</Routes>
			</BrowserRouter>
		</ApolloProvider>
	</React.StrictMode>
);
