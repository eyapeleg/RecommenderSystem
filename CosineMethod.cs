using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class CosineMethod : IPredictionMethod
    {
        private RecommenderSystem recommenderSystem;

        public CosineMethod(RecommenderSystem recommenderSystem)
        {
            // TODO: Complete member initialization
            this.recommenderSystem = recommenderSystem;
        }
    }
}
