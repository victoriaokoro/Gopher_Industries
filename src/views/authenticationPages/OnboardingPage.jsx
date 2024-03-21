import React, { useState } from 'react';
import { Link } from 'react-router-dom'
import { toast } from 'react-toastify';
import { FormView, FormViewHide } from 'grommet-icons';

export default function SignUp() {

    const [ loading, setLoading ] = useState(false);
    const [ buttonDisabled, setButtonDisabled ] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [name, setName] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleShowPassword = () => {
        setShowPassword(!showPassword);
    }

    const handleShowConfirmPassword = () => {
        setShowConfirmPassword(!showConfirmPassword);
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        setLoading(true);
        setButtonDisabled(true);
        setTimeout(() => {
            setLoading(false);
            setButtonDisabled(false);
            toast.error('APIs are not connected, but you can use the form to connect to your APIs.');
        }, 3000);
    }

  return (
    <section>
        <h3 className='text-lg font-bold text-slate-600'>Sign up as Customer</h3>
        <form 
        // ref={formRef} 
        onSubmit={handleSubmit}>
            <div className='flex flex-col justify-normal space-y-2 mt-3'>
                <label className='block text-sm font-medium text-slate-800 capitalize'>name</label>
                <input 
                    id='name'
                    name='name'
                    type='text'
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    autoComplete='on'
                    placeholder='Enter your name'
                    required
                    className='block w-full rounded-md border-0 text-slate-800 shadow-sm ring-1 ring-inset ring-gray-800 placeholder:text-gray-300 focus:ring-2 focus:ring-inset focus:ring-slate-800 text-sm sm:leading-6'
                />
            </div>
            <div className='flex flex-col justify-normal space-y-2 mt-3'>
                <label className='block text-sm font-medium text-slate-800 capitalize'>phone number</label>
                <input 
                    id='phone number'
                    name='phone number'
                    type='text'
                    value={phoneNumber}
                    onChange={(e) => setPhoneNumber(e.target.value)}
                    autoComplete='on'
                    placeholder='Enter your phone number'
                    required
                    className='block w-full rounded-md border-0 text-slate-800 shadow-sm ring-1 ring-inset ring-gray-800 placeholder:text-gray-300 focus:ring-2 focus:ring-inset focus:ring-slate-800 text-sm sm:leading-6'
                />
            </div>
            <div className='flex flex-col justify-normal space-y-2 mt-3'>
                <label className='block text-sm font-medium text-slate-800 capitalize'>email</label>
                <input 
                    id='email'
                    name='email'
                    type='email'
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete='on'
                    placeholder='Enter your email'
                    required
                    className='block w-full rounded-md border-0 text-slate-800 shadow-sm ring-1 ring-inset ring-gray-800 placeholder:text-gray-300 focus:ring-2 focus:ring-inset focus:ring-slate-800 text-sm sm:leading-6'
                />
            </div>
            <div className='flex flex-col justify-normal space-y-2 mt-3 relative'>
                <label className='block text-sm font-medium text-slate-800 capitalize'>password</label>
                <input 
                    id='password'
                    name='password'
                    type={showPassword ? 'text' : 'password'}
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete='off'
                    placeholder='Enter your password'
                    required
                    className='block w-full rounded-md border-0 text-slate-800 shadow-sm ring-1 ring-inset ring-gray-800 placeholder:text-gray-300 focus:ring-2 focus:ring-inset focus:ring-slate-800 text-sm sm:leading-6'
                />
                <span className='absolute top-5 right-0 flex inset-y-0 items-center pr-4'>
                    {showPassword ? <FormViewHide onClick={handleShowPassword} /> : <FormView onClick={handleShowPassword} />}
                </span>
            </div>
            <div className='flex flex-col justify-normal space-y-2 mt-3 relative'>
                <label className='block text-sm font-medium text-slate-800 capitalize'>confirm password</label>
                <input 
                    id='confirm password'
                    name='confirm password'
                    type={showConfirmPassword ? 'text' : 'password'}
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    autoComplete='off'
                    placeholder='Enter your password'
                    required
                    className='block w-full rounded-md border-0 text-slate-800 shadow-sm ring-1 ring-inset ring-gray-800 placeholder:text-gray-300 focus:ring-2 focus:ring-inset focus:ring-slate-800 text-sm sm:leading-6'
                />
                <span className='absolute top-5 right-0 flex inset-y-0 items-center pr-4'>
                    {showConfirmPassword ? <FormViewHide onClick={handleShowConfirmPassword} /> : <FormView onClick={handleShowConfirmPassword} />}
                </span>
            </div>
            <div>
                <button
                    type='submit'
                    disabled={buttonDisabled}
                    className={`w-full rounded-md py-1 text-sm font-semibold leading-6 shadow-sm transition duration-300 ease-in-out my-4 ${
                        (buttonDisabled)
                        ? 'w-full bg-blue-950 opacity-30 text-white cursor-not-allowed '
                        : 'w-full bg-blue-950 opacity-75 text-white hover:bg-orange-500 hover:bg-opacity-90 focus-visible:outline focus-visible:outline-2 focus-visible:outline-blue-900'
                    }`}
                    onClick={handleSubmit}
                >
                {!loading && <p>Sign up</p>}
                {loading && 
                <div className='flex flex-row justify-center space-x-2'>
                    <div>
                        <svg className="w-5 h-5 animate-spin text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                            </div>
                        <div>
                        <span class="font-medium"> Processing... </span>
                    </div>
                </div>
                }
                </button>
            </div>
            <p className='text-slate-800 text-sm'>Already have an account? <span className='text-sm font-semibold text-orange-500'><Link to={"/login"}>Sign in</Link></span></p>
        </form>
    </section>
  )
}
