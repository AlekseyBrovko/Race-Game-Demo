using System.Collections.Generic;

namespace Client
{
    struct ShopCarsComp
    {
        public Dictionary<ICar, PlayerCarSo> CarsDictionary;
        public List<ICar> Cars;
        public ICar CurrentCar;
        public PlayerCarSo CurrentCarSo;
    }
}