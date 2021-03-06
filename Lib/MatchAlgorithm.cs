﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    /// <summary>
    /// Match one user against list of other uses and return match %-percentage
    /// </summary>
    public class MatchAlgorithm
    {
        private UserModel subject;
        private List<UserModel> targets = new List<UserModel>();

        public MatchAlgorithm() { }
        public MatchAlgorithm(UserModel subject) => SetSubject(subject);

        /// <summary>
        /// Set the target subejct
        /// </summary>
        public void SetSubject(UserModel subject) => this.subject = subject;

        public void AddTarget(UserModel target) => this.targets.Add(target);

        public void AddTargets(List<UserModel> targets) => targets.ForEach(t => this.targets.Add(t));

        public static void CalculateMatchPercentage(UserModel _subject, ref List<MatchUserDtocs> _targets)
        {
            // List of subject interests
            List<string> A = new List<string>();

            _subject.Interests
                .ForEach(item => A.Add(item.Value));

            foreach (MatchUserDtocs t in _targets)
            {
                // Percentage match per person
                double percentage = 0.0;
                // Iterator
                double i = 0.0;

                // Create a string list
                List<string> B = new List<string>();
                t.Interests
                    .ForEach(item => B.Add(item.Value));

                // SUPER MATH ALGORITHM HACKER HACKERSSON
                foreach (string a in A)
                {
                    foreach (string b in B)
                    {
                        if (a == b)
                            i++;
                    }
                }

                // Double the value up since there are two lists
                if (i > 0.0)
                    i *= 2;

                percentage = Math.Round((i / (A.Count + B.Count)) * 100);
                if (Double.IsNaN(percentage))
                    percentage = 0.0;

                //Finally add to input
                t.ProtagonistMatchPercentage = percentage;
            }
        }

        //public List<double> GetInterestMatchPercentages()
        //{
        //    // Subject list
        //    List<string> a = new List<string>();
        //    List<double> results = new List<double>();

        //    this.subject.Interests.ForEach(x => a.Add(x.Value));

        //    foreach (UserModel target in targets)
        //    {
        //        // Taget list
        //        List<string> b = new List<string>();
        //        target.Interests.ForEach(x => b.Add(x.Value));

        //        int equalElements = 0;

        //        b.ForEach(x =>
        //        {
        //            if (a.Contains(x))
        //                equalElements++;
        //        });

        //        //int equalElements = a.Zip(b, (i, j) => i == j).Count(eq => eq);
        //        double equivalence = (double)equalElements / Math.Max(a.Count, b.Count);
        //        if (Double.IsNaN(equivalence))
        //            equivalence = 0.0;
        //        results.Add(equivalence);
        //    }
        //    return results;
        //}

        //public List<MatchUserDtocs> GetInterestMatchPercentages(List<MatchUserDtocs> _targets)
        //{
        //    // Subject list
        //    List<string> a = new List<string>();
        //    List<double> results = new List<double>();

        //    this.subject.Interests.ForEach(x => a.Add(x.Value));

        //    foreach (MatchUserDtocs target in _targets)
        //    {
        //        // Taget list
        //        List<string> b = new List<string>();
        //        target.Interests.ForEach(x => b.Add(x.Value));

        //        int equalElements = 0;

        //        b.ForEach(x =>
        //        {
        //            if (a.Contains(x))
        //                equalElements++;
        //        });

        //        //int equalElements = a.Zip(b, (i, j) => i == j).Count(eq => eq);
        //        double equivalence = (double)equalElements / Math.Max(a.Count, b.Count);
        //        if (Double.IsNaN(equivalence))
        //            equivalence = 0.0;
        //        results.Add(equivalence);

        //        target.ProtagonistMatchPercentage = equivalence;
        //    }
        //    return _targets;
        //}

    }
}
