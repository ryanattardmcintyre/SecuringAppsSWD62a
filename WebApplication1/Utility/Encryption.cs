using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Utility
{

    //Symmetric Encryption/Decryption //use this one to enc/dec the querystring values
       //Same key to encryption and decrypt
       //Adv: Fastest, Simpler
       //it uses Secret Key and IV + salt
       //Disadv: you have to be very careful if you are going to use this one when sending files over an insecure network

    //Asymmetric Encryption/Decryption
       //It uses two different keys: Public Key and Private key
       //Public Key: encrypt
       //Private Key: decrypt (its always recommended never to send over an insecure network the private key)
       //Adv: we now can transfer the keys securely over an insecure network
       //Disadv: this is a slow performing algorithm, you cannot encrypt/decrypt large data. normally used to encrypt base64 data


    //Hybrid Encryption/Decryption //this is need to encrypt the files uploaded by the students & decrypt the files when they are needed by the teacher
       //we r going to take the adv of both the symmetric and the asymmetric to form the hybrid
       //we are going to send over the insecure network the encrypted data which was encrypted symmetrically and therefore we are going to send the
       //encrypted symmetric keys (which were encrypted with the public key)


    //Hashing
       //1 way encryption
       //usage: in passwords
       //sha512
       //two different values should give two different digests e.g. "hello world" and "helloworld" >> 2 different digests


    //Digital Signing/verify
       //against repudiation (when an attacker tampers with the data and he denies the activity)
       //suggestion for the assignment: you generate a private & public key for every student
       //when the user uploads the file, you take the private key and you sign the file and you store the signature with the file's data
       //when the user/teacher needs to download/view the file we need to use the file's owner public key + signature, to verify whether the data is still the same 
         //   i.e belonging to that user




    public class Encryption
    {

        /// <summary>
        /// This method takes a string and returns the digest as a string
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Hash(string clearText)
        {
            //convert the clearText into an array of bytes
            //clearTexxt is a non base64 data it has to be converted this way:
            byte[] clearTextAsBytes = Encoding.UTF32.GetBytes(clearText);
            byte[] digest = Hash(clearTextAsBytes);
            //base64 data convert to/from a string we have to use this class:
            string digestAsString = Convert.ToBase64String(digest);

            return digestAsString;
        }

        public static byte[] Hash(byte [] clearTextBytes)
        {
            SHA512 myAlg = SHA512.Create();
            byte[] digest = myAlg.ComputeHash(clearTextBytes);
            return digest;
        }


    }
}
