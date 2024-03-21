import React from 'react'
import { Outlet } from 'react-router-dom';
import { GopherLogo, GuestBackground } from '../assets';

export default function GuestLayout() {

  return (
    <section className='flex flex-row justify-normal'>
        <div className='relative w-full h-full'>
        <div className='absolute inset-0 bg-gradient-to-t from-black opacity-80 from-0%'></div>
            <img 
                src={GuestBackground}
                alt='background_image'
                className='h-screen w-full object-cover'
            />
            <div className='absolute bottom-10 w-full px-40 text-2xl font-bold text-white text-center'>
                Your ultimate solution for delicious, nutritious meals delivered right to your doorstep! Say goodbye to meal prep stress and hello to wholesome, convenient dining.
                <br />
                <span className='text-amber-600 font-serif'>Gopher Industries</span>
            </div>
        </div>
        <div className='flex justify-center w-1/2 h-full'>
            <div className='flex flex-col justify-center space-y-6 w-full mx-10 my-5'>
                <div className='flex flex-row justify-normal space-x-3'>
                    <img 
                        src={GopherLogo}
                        alt='Logo'
                        className='w-10 h-10'
                    />
                    <h1 className='capitalize text-2xl font-bold font-serif opacity-70'>Gopher Industries</h1>
                </div>
                <Outlet />
            </div>
        </div>
    </section>
  )
}
